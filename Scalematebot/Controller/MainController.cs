using Scalemate;
using Scalematebot.Model;
using Scalematebot.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalematebot.Controller
{
    public class MainController
    {
        #region Properties
        public MainModel Model { get; private set; }
        public MainView View { get; private set; }
        public Tester Mate { get; private set; }
        public string Question { get; private set; }
        public string[] Questions { get; private set; }
        public string[] Answers { get; private set; }
        public int Index { get; private set; }
        public bool Running { get; private set; }
        #endregion

        #region Constructor
        public MainController(MainModel model, MainView view)
        {
            Model = model;
            View = view;
            Mate = new Tester(Model, "procast-student-pt", View.Id);
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
