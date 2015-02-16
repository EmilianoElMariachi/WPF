using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses
{
    interface IRecorderInterface
    {
        bool CanRecordPropertyChange { get; }

        void RecordPropertyChange(IRecordedPropertyInfo recordedPropertyInfo, IRevertibleCommand revertibleCommand);


    }

    public interface IRecordedPropertyInfo
    {

        uint DelayMs { get; }

    }
}
