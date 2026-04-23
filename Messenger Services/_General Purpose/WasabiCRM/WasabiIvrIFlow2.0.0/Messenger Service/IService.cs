
//==========================================================================================================

// Copyright © IFM Infomaster. All rights reserved.

//==========================================================================================================

// Author    : Marco Solinas [MSO]
// Date      : Mar 2017
// Revisions : 

//==========================================================================================================

#region Namespaces

using System.Runtime.InteropServices;

#endregion Namespaces

namespace Ifm.Components.Messenger.Blocks.Interfaces {
    [ComVisible(true)]
    [GuidAttribute("3740F440-34CE-42F8-A922-97173FDE18A1")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IService {        
        void inizialize(dynamic voiceHandlers);
        void execute();

        void PlayQueueDone(long queueResult);
        void AsrRecognitionEvent(int eventType);
    }
}
