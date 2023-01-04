using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HamdmadeNews.Infrastructure
{
    public interface IScrapperService
    {
        Task DoScrap();
        Task SendToTelegram();
    }
}
