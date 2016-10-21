using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Shijuhua
{
    class AssociationUriMapper : UriMapperBase
    {
        private string tempUri;

        public override Uri MapUri(Uri uri)
        {
            tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            //验证Uri，是否为已注册的关联
            if (tempUri.StartsWith("/Protocol?encodedLaunchUri=shijuhua:"))
            {
                string uriMsg = tempUri.Substring(36);
                return new Uri("/MainPage.xaml?Msg=" + uriMsg, UriKind.Relative);
            }
            return uri;
        }
    }
}
