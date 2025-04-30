using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
namespace AlanJayApp.wwwroot.Messages
{
    public class VinScannedMessage : ValueChangedMessage<string>
    {
        public VinScannedMessage(string vin) : base(vin) { }
    }
}
