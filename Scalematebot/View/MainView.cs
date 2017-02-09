using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalematebot.View
{
    public class MainView
    {
        #region Quiz parameters
        public string[] Questions { get; private set; } = new[]
        {
            "Are you ok?",
            "Do you want to be here?",
            "Is there anything I can do for you?"
        };

        public string[] Answers { get; private set; } = new[]
        {
            "Yes",
            "No"
        };

        public int Index { get; private set; }
        public bool Running { get; private set; }
        #endregion

        #region Methods
        public void Start()
        {
            Index = 0;
            Running = true;
        }

        public bool Ended()
        {
            return !Running;
        }

        public void Next()
        {
            Index++;
            Running = Index < Questions.Length;
        }
        #endregion
    }
}
