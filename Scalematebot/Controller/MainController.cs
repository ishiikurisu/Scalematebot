using Scalemate;
using Scalematebot.Model;
using Scalematebot.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scalematebot.Controller
{
    /// <summary>
    /// A test on this controller is performed by running `Start` and then acting 
    /// according to each step. After the steps are completed, don't forget to `Save`.
    /// 
    /// The instructions step will only display the instructions message. 
    /// 
    /// The survey step will do the survey. Try to get the next survey question by running
    /// `NextSurveyQuestion`. If the gotten question is null, then it is time for the next
    /// step. Else, display the question and collect the answer using `Set`.
    /// 
    /// The test step will run Scalemate as we know it. Get the `Options` and the `Question`,
    /// display them, and go to the `NextQuestion`. This controller will end when there are 
    /// no more questions to display.
    /// </summary>
    public class MainController
    {
        #region Properties
        public MainModel Model { get; private set; } = null;
        public MainView View { get; private set; } = null;
        public Tester Mate { get; private set; } = null;
        public string Question { get; private set; } = null;
        public Queue<string> Survey { get; private set; } = null;
        public List<string> SurveyAnswers { get; private set; } = null;
        public string[] Questions { get; private set; } = null;
        public string[] Options { get; private set; } = null;
        public List<string> Answers { get; private set; } = null;
        public int Index { get; private set; } = -1;
        public bool Running { get; private set; } = false;
        public Queue<string> Steps { get; private set; } = null;
        public string CurrentStep { get; private set; } = null;
        public bool FirstSurvey = true;
        public bool FirstQuestion = true;
        public bool LastQuestion = false;
        #endregion

        #region Constructor
        public MainController(MainModel model, MainView view)
        {
            // MVC setup
            Model = model;
            View = view;
            Mate = new Tester(Model, "procast-student-pt", View.Id);

            // Setting steps
            Steps = GetSteps();
            CurrentStep = NextStep();
        }
        #endregion

        #region General stuff
        public void Start()
        {
            // Preparing test
            Question = Mate.Question;
            Options = Mate.Options.Take(Mate.NoOptions).ToArray();
            Answers = new List<string>();
            FirstQuestion = true;

            // Preparing survey
            FirstSurvey = true;
            Survey = new Queue<string>(Mate.SurveyQuestions);
            SurveyAnswers = new List<string>();
        }

        public void Set(string step, string answer)
        {
            switch (step)
            {
                case "survey":
                    if (FirstSurvey)
                        FirstSurvey = false;
                    else
                        SurveyAnswers.Add(answer);
                    break;
                case "test":
                    if (FirstQuestion)
                        FirstQuestion = false;
                    else
                        Mate.Listen(int.Parse(answer[0].ToString()) - 1);
                    break;
            }
        }

        public void Save()
        {
            // Cleaning answers
            if (Mate.SurveyQuestions != null)
                Mate.SurveyAnswers = SurveyAnswers.ToArray();

            // Storing answers on memory
            var results = Mate.CalculateResults();
            Console.WriteLine(results);
        }

        public void NextQuestion()
        {
            if (!LastQuestion)
            {
                Mate.Continue();
                Question = Mate.Question;
                Options = Mate.Options.Take(Mate.NoOptions).ToArray();
            }
        }

        public bool Ended()
        {
            if (Mate.Ended)
                if (LastQuestion)
                    return true;
                else
                    LastQuestion = true;
            return false;
        }
        #endregion

        #region Step stuff
        public string GetInstructions()
        {
            return Mate.BeginningInstructions.Aggregate("", (box, it) => $"{box} {it}");
        }

        public string NextSurveyQuestion()
        {
            return (Survey.Count > 0) ? Survey.Dequeue() : null;
        }

        public Queue<string> GetSteps()
        {
            Queue<string> steps = new Queue<string>();

            if (Mate.SurveyQuestions != null)
                steps.Enqueue("survey");
            if (Mate.BeginningInstructions != null)
                steps.Enqueue("instructions");
            steps.Enqueue("test");

            return steps;
        }

        public string NextStep()
        {
            return (Steps.Count > 0) ? Steps.Dequeue() : null;
        }

        public string GetThanks()
        {
            return (Mate.EndingInstructions != null) ? 
                Mate.EndingInstructions.Aggregate("", (box, it) => $"{box} {it}") : 
                "Obrigada por participar!";
        }
        #endregion
    }
}
