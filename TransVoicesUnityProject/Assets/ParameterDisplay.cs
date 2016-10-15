using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SingleParameterDisplay {

   public SpeechParameterType type;
   public Text                numberText;
   public Slider              slider;
}

public class ParameterDisplay: MonoBehaviour {
   
   public List< SingleParameterDisplay > parameterDisplays;

   public void UpdateDisplay( List< SpeechParameter > parameters ) {

      foreach (var parameter in parameters) {

         float score = FindObjectOfType< GameManager >().ScoreForResultParameter( parameter );
         UpdateDisplay( parameter.type, parameter.value, score );
      }
   }

   public void UpdateDisplay( SpeechParameterType type, float value, float? score=null ) {

      SingleParameterDisplay element = parameterDisplays.FirstOrDefault( e => e.type == type );

      if (element == null) { return; }

      element.numberText.text = value.ToString( "0.00" );
      if (element.slider && score.HasValue) { element.slider.value = score.Value; }
   }
}
