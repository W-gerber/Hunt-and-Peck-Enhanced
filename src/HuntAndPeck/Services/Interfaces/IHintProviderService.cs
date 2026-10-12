using System;
using HuntAndPeck.Models;

namespace HuntAndPeck.Services.Interfaces
{

    public interface IHintProviderService
    {

        HintSession EnumHints();

        HintSession EnumHints(IntPtr handle);
    }
}
