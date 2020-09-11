using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpressBase.Mobile.Data;
using ExpressBase.Mobile.Droid.Helpers.Script;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Helpers.Script;
using Java.Util.Functions;
using Mono.CSharp;

[assembly: Xamarin.Forms.Dependency(typeof(CSharpScriptHelper))]

namespace ExpressBase.Mobile.Droid.Helpers.Script
{
    public class EbEvaluator
    {
        public string Validate()
        {
            return "hello";
        }
    }

    public class CSharpScriptHelper : IScriptHelper
    {
        private Evaluator evaluator;

        public CSharpScriptHelper()
        {
            InitializeEvaluator();
        }

        private void InitializeEvaluator()
        {
            var settings = new CompilerSettings()
            {
                StdLib = true
            };

            var reportPrinter = new ConsoleReportPrinter();
            var ctx = new CompilerContext(settings, reportPrinter);

            evaluator = new Evaluator(ctx);
            evaluator.ReferenceAssembly(Assembly.GetExecutingAssembly());
        }

        public T Evaluate<T>(string defenition, string expr)
        {
            try
            {
                evaluator.Run(defenition);

                return (T)evaluator.Evaluate(expr);
            }
            catch (Exception ex)
            {
                EbLog.Info("Error at CSharpScripting evaluate");
                EbLog.Info(ex.Message);
            }

            return default;
        }
    }
}