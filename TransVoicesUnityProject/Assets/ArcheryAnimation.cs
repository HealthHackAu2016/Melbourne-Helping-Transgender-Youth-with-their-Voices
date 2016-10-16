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

   public void Fire( float differenceSum ) {

      StartCoroutine( AnimationRoutine( differenceSum ) );
   }

   IEnumerator AnimationRoutine( float differenceSum ) {

      animating       = true;
      float startTime = Time.time;
      gameManager.startTurnButton.interactable = false;
      
      bool    hit       = differenceSum <= gameManager.baselineDifferenceSum;  // if outside radius, miss the target
      float   duration  = hit ? arcDuration : arcDuration * 2.0f;
      float   hitRadius = Mathf.LerpUnclamped( 0.0f, targetRadius, differenceSum / gameManager.baselineDifferenceSum );
      Vector3 hitPoint  = target.position + (Vector3) (Random.insideUnitCircle.normalized * hitRadius);

      while (Time.time < startTime + duration) {  // animate arrow flight

         float t        = (Time.time - startTime) / arcDuration;
         float extraY   = (1.0f - Mathf.Pow( t * 2.0f - 1.0f, 2.0f )) * arcHeight;  // parabola height
         arrow.position = Vector3.LerpUnclamped( spawnPoint.position, hitPoint, t ) + Vector3.up * extraY;
         
         yield return null;
      }
      if (hit) {
         Vector3    decalPoint    = hitPoint + Vector3.back * 0.05f;  // position hit decal in front of target
         GameObject decalInstance = Instantiate( hitDecal, decalPoint, Quaternion.identity ) as GameObject;
         hitDecalInstances.Add( decalInstance );
         arrow.position = hitPoint;
      } else {
         arrow.position = spawnPoint.position;
      }

      if (hitDecalInstances.Count > maxHitDecals) {  // remove earliest hit decal if over limit

         Destroy( hitDecalInstances[ 0 ] );
         hitDecalInstances.RemoveAt( 0 );
      }
      animating = false;
      gameManager.startTurnButton.interactable = true;
   }
}
