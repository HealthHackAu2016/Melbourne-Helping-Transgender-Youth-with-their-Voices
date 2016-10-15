using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager: MonoBehaviour {

   public float            baselineScore = -1.0f, goalScore = -1.0f;
   public Text             baselineScoreNumber, goalScoreNumber,
                           lastScoreText, turnsText, highestScoreText, averageScoreText;
   public Button           startButton;
   public ArcheryAnimation archeryAnimation;

   List< float > scores = new List< float >();
   float         highestScore;
   
   public void SetBaselineScore( float score ) {

      baselineScore            = score;
      baselineScoreNumber.text = score.ToString("0.00");
   }
   
   public void SetGoalScore( float score ) {

      goalScore            = score;
      goalScoreNumber.text = score.ToString("0.00");
   }

   public void StartTurn() {
      
   }

   public void GotTurnResult( float resultScore ) {

      archeryAnimation.Fire( resultScore );
      scores.Add( resultScore );

      lastScoreText.text    = resultScore.ToString("0.00");
      turnsText.text        = scores.Count.ToString();
      averageScoreText.text = scores.Average().ToString("0.00");

      if (resultScore > highestScore) {

         highestScore          = resultScore;
         highestScoreText.text = resultScore.ToString("0.00");
      }
   }

   public void TestTurn() {

      float score = Random.Range( baselineScore, goalScore );
      GotTurnResult( score );
   }
}
