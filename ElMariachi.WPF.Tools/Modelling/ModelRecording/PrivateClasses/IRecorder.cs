using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses
{
    interface IRecorder
    {
        bool CanRecordPropertyChange { get; }

        void RecordPropertyChange(Property property, IRevertibleCommand revertibleCommand);


    }

    internal class Property
    {

        public int Delay { get; set; }

        public string RecordGroupName { get; set; }

    }
}
