using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour {

    public float speed;

    private GameObject[] patrolPoints;

    int randomPoint;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        patrolPoints = GameObject.FindGameObjectsWithTag("point");
        randomPoint = Random.Range(0, patrolPoints.Length);
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        animator.transform.position = Vector2.MoveTowards(animator.transform.position, patrolPoints[randomPoint].transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(animator.transform.position, patrolPoints[randomPoint].transform.position) < 0.1f)
        {
            randomPoint = Random.Range(0, patrolPoints.Length);
        }

    }

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

}
