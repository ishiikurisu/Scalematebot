﻿using Scalemate;
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
        public Queue<string> Survey { get; private set; }
        public string[] Questions { get; private set; }
        public string[] Answers { get; private set; }
        public int Index { get; private set; }
        public bool Running { get; private set; }
        public Queue<string> Steps { get; private set; }
        public string CurrentStep { get; private set; }
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

        public void Start()
        {
            // Preparing test
            Question = Mate.Question;
            Answers = Mate.Options.Take(Mate.NoOptions).ToArray();

            // Preparing survey
            Survey = new Queue<string>(Mate.SurveyQuestions);
        }
        #endregion

        #region Test methods
        public void Set(string step, string answer)
        {
            switch (step)
            {
                case "survey":
                case "test":
                    Console.WriteLine($"{step}: {answer}");
                    break;
            }
            // TODO Store answer
            // Don't store the first answer!
        }

        public bool Ended()
        {
            return Mate.Ended;
        }

        public void NextQuestion()
        {
            Mate.Continue();
            Start();
        }

        public string GetInstructions()
        {
            return Mate.BeginningInstructions.Aggregate("", (box, it) => $"{box} {it}");
        }
        #endregion

        #region Survey methods

        public string NextSurveyQuestion()
        {
            return (Survey.Count > 0) ? Survey.Dequeue() : null;
        }
        #endregion

        #region Steps methods
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
