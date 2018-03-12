using UnityEngine;

public class SetInteger : StateMachineBehaviour {

	public string integerName;
	public int integerValue;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetInteger(integerName, integerValue);
	}
}
