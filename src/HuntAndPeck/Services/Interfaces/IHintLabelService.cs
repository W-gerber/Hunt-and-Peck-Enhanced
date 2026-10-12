using System.Collections.Generic;

namespace HuntAndPeck.Services.Interfaces
{
    public interface IHintLabelService
    {

        IList<string> GetHintStrings(int count);
    }
}
