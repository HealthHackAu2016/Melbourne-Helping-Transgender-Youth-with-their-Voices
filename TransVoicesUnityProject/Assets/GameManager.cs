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

   public List< SpeechParameter > femaleGoalParameters, maleGoalParameters;
   public ParameterDisplay        goalParametersDisplay, baselineParametersDisplay,
                                  lastResultsDisplay, bestResultsDisplay;
   public Text                    turnsText;
   public Button                  startGameButton, startTurnButton;
   public Toggle                  femaleToggle;
   public GameObject              calibrationPanel, scoringPanel, recordingOverlay;
   public ArcheryAnimation        archeryAnimation;
   public bool                    useDummyResults;

   internal List< SpeechParameter > goalParameters, baselineParameters, bestResults;
   internal float baselineDifferenceSum;  // sum of differences of values between baseline and goal parameters
   internal int   numTurns = 0;

   List< List< SpeechParameter > > pastResults = new List< List< SpeechParameter > >();

   SpeechParameterType? currResultType;
   float?               currResultValue;
   bool                 resultReady = false;

   bool waitingForResult;

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
      
      if (useDummyResults) { baselineParameters = GetDummyResults();       }
      else                 { StartCoroutine( RequestRecording() );         }

      startGameButton.interactable = true;
   }

   public void ReturnRecordedParameterType( string type ) {

      currResultType = (SpeechParameterType) System.Enum.Parse( typeof( SpeechParameterType ), type );
   }

   public void ReturnRecordedParameterValue( float value ) {

      currResultValue = value;
   }

   public void ReturnResultReady( bool result ) {

      resultReady = result;
   }

   public IEnumerator RequestRecording() {

      Application.ExternalEval( "window.StartAttempt" );
      recordingOverlay.SetActive( true );

      do {
         Application.ExternalEval( "SendMessage( 'GameManager', 'ReturnResultReady', window.CheckResult() )" );
         yield return new WaitForSeconds( 0.2f );

      } while (!resultReady);

      recordingOverlay.SetActive( false );
      var results = GetRecordedParameters();

      if (calibrationPanel.activeSelf) {
         baselineParameters = results;
         startGameButton.interactable = true;
      } else {
         GotTurnResult( results );
      }
   }

   public List< SpeechParameter > GetRecordedParameters() {

      Application.ExternalEval( "SendMessage( 'GameManager', 'ReturnRecordedParameterValue', window.CheckPitch() )" );

      var result = new List< SpeechParameter >() {  // dummy results until things are hooked up
         new SpeechParameter( SpeechParameterType.Amplitude, 0.5f ),
         new SpeechParameter( SpeechParameterType.Pitch,     currResultValue.Value )
      };
      currResultType  = null;
      currResultValue = null;

      return result;
   }

   public List< SpeechParameter > GetDummyResults() {

      return new List< SpeechParameter >() {  // dummy results until things are hooked up
         new SpeechParameter( SpeechParameterType.Amplitude, Random.Range( 0.0f, 1.0f ) ),
         new SpeechParameter( SpeechParameterType.Pitch,     Random.Range( 0.0f, 1.0f ) )
      };
   }

   public void StartGame() {

      if (femaleToggle.isOn) { goalParameters = femaleGoalParameters; }
      else                   { goalParameters = maleGoalParameters;   }
      
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

   public void TestCallFromJavascript( string message ) {

      Debug.Log( "In Unity; received message from javascript on frame " + Time.frameCount + ": " + message );
   }
}
