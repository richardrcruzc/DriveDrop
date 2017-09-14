using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Mobile.Services
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
