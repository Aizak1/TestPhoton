using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour {

    private Player player;
    public float speed;


	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (player != null)
        {
            animator.transform.position = Vector2.MoveTowards(animator.transform.position, player.transform.position, speed * Time.deltaTime);
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}


}
