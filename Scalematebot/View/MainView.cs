using Scalemate;
using Scalematebot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalematebot.View
{
    public class MainView
    {
        #region Properties
        public Tester Mate { get; private set; }
        public string Question { get; private set; }
        public string[] Questions { get; private set; }
        public string[] Answers { get; private set; }
        public int Index { get; private set; }
        public bool Running { get; private set; }
        #endregion

        #region Constructor
        public MainView(string id)
        {
            Mate = new Tester(new MainModel(), "procast-student-pt", id);
        }
        #endregion

        #region Methods
        public void Start()
        {
            Question = Mate.Question;
            Answers = Mate.Options.Take(Mate.NoOptions).ToArray();
        }

        public void Set(string answer)
        {
            // TODO Store answer
        }

        public bool Ended()
        {
            return Mate.Ended;
        }

        public void Next()
        {
            Mate.Continue();
            Start();
        }
        #endregion
    }
}
