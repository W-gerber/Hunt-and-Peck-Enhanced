using HuntAndPeck.Extensions;
using HuntAndPeck.Models;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using UIAutomationClient;

namespace HuntAndPeck.Services
{
    internal class UiAutomationHintProviderService : IHintProviderService, IDebugHintProviderService
    {
        private readonly IUIAutomation _automation = new CUIAutomation();
        private readonly IUIAutomationCacheRequest _cacheRequest;
        private readonly IUIAutomationCondition _condition;

        public UiAutomationHintProviderService()
        {
            // Fetch everything CreateHint needs in a single cross-process call
            // instead of 1 + up to 6 calls per element.
            _cacheRequest = _automation.CreateCacheRequest();
            _cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            _cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsValuePatternAvailablePropertyId);
            _cacheRequest.AddProperty(UIA_PropertyIds.UIA_ValueIsReadOnlyPropertyId);
            _cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsRangeValuePatternAvailablePropertyId);
            _cacheRequest.AddProperty(UIA_PropertyIds.UIA_RangeValueIsReadOnlyPropertyId);
            _cacheRequest.AddPattern(UIA_PatternIds.UIA_InvokePatternId);
            _cacheRequest.AddPattern(UIA_PatternIds.UIA_TogglePatternId);
            _cacheRequest.AddPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            _cacheRequest.AddPattern(UIA_PatternIds.UIA_ExpandCollapsePatternId);

            var enabledControls = _automation.CreateAndCondition(
                _automation.ControlViewCondition,
                _automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsEnabledPropertyId, true));
            _condition = _automation.CreateAndCondition(
                enabledControls,
                _automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsOffscreenPropertyId, false));
        }

        public HintSession EnumHints()
        {
            var foregroundWindow = User32.GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
            {
                return null;
            }
            return EnumHints(foregroundWindow);
        }

        public HintSession EnumHints(IntPtr hWnd)
        {
            var sw = Stopwatch.StartNew();
            var session = EnumWindowHints(hWnd, CreateHint);
            Debug.WriteLine("Enumeration of hints took {0} ms", sw.ElapsedMilliseconds);
            return session;
        }

        public HintSession EnumDebugHints()
        {
            var foregroundWindow = User32.GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
            {
                return null;
            }
            return EnumDebugHints(foregroundWindow);
        }

        public HintSession EnumDebugHints(IntPtr hWnd)
        {
            return EnumWindowHints(hWnd, CreateDebugHint);
        }

        private HintSession EnumWindowHints(IntPtr hWnd, Func<IntPtr, Rect, IUIAutomationElement, Hint> hintFactory)
        {
            var result = new List<Hint>();

            var rawWindowBounds = new RECT();
            User32.GetWindowRect(hWnd, ref rawWindowBounds);
            Rect windowBounds = rawWindowBounds;

            foreach (var element in EnumElements(hWnd))
            {
                try
                {
                    var r = element.CachedBoundingRectangle;
                    if (r.right <= r.left || r.bottom <= r.top)
                    {
                        continue;
                    }

                    var niceRect = new Rect(new Point(r.left, r.top), new Point(r.right, r.bottom));
                    if (niceRect.PhysicalToLogicalRect(hWnd).IsEmpty)
                    {
                        continue;
                    }

                    var hint = hintFactory(hWnd, niceRect.ScreenToWindowCoordinates(windowBounds), element);
                    if (hint != null)
                    {
                        result.Add(hint);
                    }
                }
                catch (Exception)
                {
                }
            }

            return new HintSession
            {
                Hints = result,
                OwningWindow = hWnd,
                OwningWindowBounds = windowBounds,
            };
        }

        private IEnumerable<IUIAutomationElement> EnumElements(IntPtr hWnd)
        {
            if (!User32.IsWindow(hWnd))
            {
                yield break;
            }

            var elementArray = _automation.ElementFromHandle(hWnd)
                .FindAllBuildCache(TreeScope.TreeScope_Descendants, _condition, _cacheRequest);
            if (elementArray == null)
            {
                yield break;
            }

            for (var i = 0; i < elementArray.Length; ++i)
            {
                yield return elementArray.GetElement(i);
            }
        }

        private static bool CachedBool(IUIAutomationElement element, int propertyId)
        {
            return element.GetCachedPropertyValue(propertyId) is bool b && b;
        }

        private Hint CreateHint(IntPtr owningWindow, Rect hintBounds, IUIAutomationElement automationElement)
        {
            try
            {
                // Cached pattern objects still perform their actions live.
                if (automationElement.GetCachedPattern(UIA_PatternIds.UIA_InvokePatternId) is IUIAutomationInvokePattern invokePattern)
                {
                    return new UiAutomationInvokeHint(owningWindow, invokePattern, hintBounds);
                }

                if (automationElement.GetCachedPattern(UIA_PatternIds.UIA_TogglePatternId) is IUIAutomationTogglePattern togglePattern)
                {
                    return new UiAutomationToggleHint(owningWindow, togglePattern, hintBounds);
                }

                if (automationElement.GetCachedPattern(UIA_PatternIds.UIA_SelectionItemPatternId) is IUIAutomationSelectionItemPattern selectPattern)
                {
                    return new UiAutomationSelectHint(owningWindow, selectPattern, hintBounds);
                }

                if (automationElement.GetCachedPattern(UIA_PatternIds.UIA_ExpandCollapsePatternId) is IUIAutomationExpandCollapsePattern expandCollapsePattern)
                {
                    return new UiAutomationExpandCollapseHint(owningWindow, expandCollapsePattern, hintBounds);
                }

                if ((CachedBool(automationElement, UIA_PropertyIds.UIA_IsValuePatternAvailablePropertyId)
                        && !CachedBool(automationElement, UIA_PropertyIds.UIA_ValueIsReadOnlyPropertyId))
                    || (CachedBool(automationElement, UIA_PropertyIds.UIA_IsRangeValuePatternAvailablePropertyId)
                        && !CachedBool(automationElement, UIA_PropertyIds.UIA_RangeValueIsReadOnlyPropertyId)))
                {
                    return new UiAutomationFocusHint(owningWindow, automationElement, hintBounds);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private DebugHint CreateDebugHint(IntPtr owningWindow, Rect hintBounds, IUIAutomationElement automationElement)
        {
            var programmaticNames = new List<string>();

            foreach (var pn in UiAutomationPatternIds.PatternNames)
            {
                try
                {
                    if (automationElement.GetCurrentPattern(pn.Key) != null)
                    {
                        programmaticNames.Add(pn.Value);
                    }
                }
                catch (Exception)
                {
                }
            }

            return programmaticNames.Any()
                ? new DebugHint(owningWindow, hintBounds, programmaticNames)
                : null;
        }
    }
}
