using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcheryAnimation: MonoBehaviour {

   public Transform   arrow, target, spawnPoint;
   public GameObject  hitDecal;
   public float       arcDuration, arcHeight, targetRadius;
   public int         maxHitDecals;
   public GameManager gameManager;

   List< GameObject > hitDecalInstances = new List< GameObject >();
   
   public bool animating { get; private set; }

   public void Fire( float score ) {

      StartCoroutine( AnimationRoutine( score ) );
   }

   IEnumerator AnimationRoutine( float score ) {

      animating       = true;
      float startTime = Time.time;
      gameManager.startButton.interactable = false;

      float   normalizedScore = Mathf.InverseLerp( gameManager.baselineScore, gameManager.goalScore, score );
      float   hitRadius       = Mathf.Lerp( targetRadius, 0.0f, normalizedScore );
      Vector3 hitPoint        = target.position + (Vector3) (Random.insideUnitCircle.normalized * hitRadius);

      while (Time.time < startTime + arcDuration) {

         float t        = (Time.time - startTime) / arcDuration;
         float extraY   = (1.0f - Mathf.Pow( t * 2.0f - 1.0f, 2.0f )) * arcHeight;
         arrow.position = Vector3.Lerp( spawnPoint.position, hitPoint, t ) + Vector3.up * extraY;
         
         yield return null;
      }
      Vector3    decalPoint    = hitPoint + Vector3.back * 0.05f;
      GameObject decalInstance = Instantiate( hitDecal, decalPoint, Quaternion.identity ) as GameObject;
      hitDecalInstances.Add( decalInstance );

      if (hitDecalInstances.Count > maxHitDecals) {

         Destroy( hitDecalInstances[ 0 ] );
         hitDecalInstances.RemoveAt( 0 );
      }
      arrow.position = hitPoint;
      animating      = false;
      gameManager.startButton.interactable = true;
   }
}
