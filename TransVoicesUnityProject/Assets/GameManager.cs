using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SpeechParameter{

   public SpeechParameterType type;
   public float               value;

   public SpeechParameter( SpeechParameterType type, float value ) {

      this.type  = type;
      this.value = value;
   }
}

public enum SpeechParameterType { Amplitude, Pitch }

public class GameManager: MonoBehaviour {

   const string jsCheckResultFunction  = "window.CheckResult()";
   const string jsCheckPitchFunction   = "window.CheckPitch()";
   const string jsStartAttemptFunction = "window.StartAttempt()";
   const string jsGetGoalFunction      = "window.GetTarget()";

   static object currResult;

   public List< SpeechParameter > femaleGoalParameters, maleGoalParameters;
   public ParameterDisplay        goalParametersDisplay, baselineParametersDisplay,
                                  lastResultsDisplay, bestResultsDisplay;
   public Text                    turnsText;
   public Button                  startGameButton, startTurnButton;
   public Toggle                  femaleToggle;
   public GameObject              calibrationPanel, scoringPanel, recordingOverlay, genderSwitch;
   public ArcheryAnimation        archeryAnimation;
   public bool                    useDummyResults, getGoalFromJavascript;

   internal List< SpeechParameter > goalParameters, baselineParameters, bestResults;
   internal float baselineDifferenceSum;  // sum of differences of values between baseline and goal parameters
   internal int   numTurns = 0;

   List< List< SpeechParameter > > pastResults = new List< List< SpeechParameter > >();

   void Awake() {

      if (getGoalFromJavascript) { genderSwitch.SetActive( false ); }
   }

   public float DifferenceSumFromGoal( List< SpeechParameter > parameters ) {

      float differenceSum = 0.0f;
      
      foreach (var parameter in parameters) {
         differenceSum += Mathf.Abs( parameter.value - goalParameters.First( sp => sp.type == parameter.type ).value );
      }

      return differenceSum;
   }

   // compare two sets of parameters and return the one closest to the goal parameters
   public List< SpeechParameter > BestOfTwoResults( List< SpeechParameter > a, List< SpeechParameter > b ) {

      if (DifferenceSumFromGoal( a ) > DifferenceSumFromGoal( b )) { return b; }

      return a;
   }

   // rate a single result with a score from 0 (or below) to 1 based on comparing to the baseline and goal parameters
   public float ScoreForResultParameter( SpeechParameter result ) {

      float baselineValue = baselineParameters.First( sp => sp.type == result.type ).value;
      float goalValue     = goalParameters.First( sp => sp.type == result.type ).value;

      return Mathf.InverseLerp( baselineValue, goalValue, result.value );
   }

   public void RecordCalibration() {
      
      if (useDummyResults) { baselineParameters = GetDummyResults(); }
      else                 { StartCoroutine( RequestRecording() );   }

      startGameButton.interactable = true;
   }

   public void ReturnValueFromJavascript( object value ) {

      currResult = value;
   }
   
   public static object GetValueFromJavascript( string evaluation ) {

      Application.ExternalEval( "SendMessage( 'GameManager', 'ReturnRecordedParameterValue', " + evaluation + " )" );
      return currResult;
   }

   public IEnumerator RequestRecording() {

      Application.ExternalEval( jsStartAttemptFunction );
      recordingOverlay.SetActive( true );
      
      while (!(bool) GetValueFromJavascript( jsCheckResultFunction )) { yield return new WaitForSeconds( 0.2f ); }

      recordingOverlay.SetActive( false );
      var results = GetRecordedParameters();

      if (calibrationPanel.activeSelf) {

         baselineParameters           = results;
         startGameButton.interactable = true;
      }
      else {
         GotTurnResult( results );
      }
   }

   public List< SpeechParameter > GetRecordedParameters() {

      float pitch = (float) GetValueFromJavascript( jsCheckPitchFunction );

      var result = new List< SpeechParameter >() {  // dummy results until things are hooked up
         new SpeechParameter( SpeechParameterType.Amplitude, 0.5f  ),
         new SpeechParameter( SpeechParameterType.Pitch,     pitch )
      };

      return result;
   }

   public List< SpeechParameter > GetDummyResults() {

      return new List< SpeechParameter >() {  // dummy results until things are hooked up
         new SpeechParameter( SpeechParameterType.Amplitude, Random.Range( 0.0f, 1.0f ) ),
         new SpeechParameter( SpeechParameterType.Pitch,     Random.Range( 0.0f, 1.0f ) )
      };
   }

   public void StartGame() {

      if (getGoalFromJavascript && !useDummyResults) {

         float goalPitch = (float) GetValueFromJavascript( jsGetGoalFunction );

         goalParameters = new List< SpeechParameter >() {  // dummy results until things are hooked up
            new SpeechParameter( SpeechParameterType.Amplitude, 0.5f      ),
            new SpeechParameter( SpeechParameterType.Pitch,     goalPitch )
         };
      }
      else {
         if (femaleToggle.isOn) { goalParameters = femaleGoalParameters; }
         else                   { goalParameters = maleGoalParameters;   }
      }
      
      baselineDifferenceSum = DifferenceSumFromGoal( baselineParameters );

      baselineParametersDisplay.UpdateDisplay( baselineParameters );
      goalParametersDisplay.UpdateDisplay( goalParameters );

      calibrationPanel.SetActive( false );
      scoringPanel.SetActive( true );
   }

   public void StartTurn() {
      
   }

   // process result of turn and start animation
   public void GotTurnResult( List< SpeechParameter > results ) {

      archeryAnimation.Fire( DifferenceSumFromGoal( results ) );
      pastResults.Add( results );

      lastResultsDisplay.UpdateDisplay( results );
      numTurns++;
      turnsText.text = numTurns.ToString();

      if (bestResults == null) { bestResults = results;                                  }
      else                     { bestResults = BestOfTwoResults( bestResults, results ); }

      bestResultsDisplay.UpdateDisplay( bestResults );
   }

   public void TestTurn() {
      
      var results = GetDummyResults();
      //var results = GetRecordedParameters();
      GotTurnResult( results );
   }
}
