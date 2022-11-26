using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{

	[Header("Animator")]
	public Animator animator;
	int horizontal;
	int vertical;

	// Start is called before the first frame update
	void Awake()
	{
		animator = GetComponent<Animator>();
		horizontal = Animator.StringToHash("Horizontal");
		vertical = Animator.StringToHash("Vertical");
	}

	public void PlayTargetAnimation(string targetAnimation, bool interaction)
	{
		animator.SetBool("isInteracting", interaction);
		animator.CrossFade(targetAnimation, 0.2f);
	}

	public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
	{
		//Animation Snapping
		float snappedHorizontal;
		float snappedVertical;

		#region Snapped Horizontal
		if (horizontalMovement > 0 && horizontalMovement < 0.55f)
			snappedHorizontal = 0.5f;
		else if (horizontalMovement > 0.55f)
			snappedHorizontal = 1f;
		else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
			snappedHorizontal = -0.5f;
		else if (horizontalMovement < -0.55f)
			snappedHorizontal = -1f;
		else
			snappedHorizontal = 0;
		#endregion
		#region Snapped Vertical
		if (verticalMovement > 0 && verticalMovement < 0.55f)
			snappedVertical = 0.5f;
		else if (verticalMovement > 0.55f)
			snappedVertical = 1f;
		else if (verticalMovement < 0 && verticalMovement > -0.55f)
			snappedVertical = -0.5f;
		else if (verticalMovement < -0.55f)
			snappedVertical = -1f;
		else
			snappedVertical = 0;
		#endregion

		if (isSprinting)
		{
			snappedHorizontal = horizontalMovement;
			snappedVertical = 2f;
		}

		animator.SetFloat(horizontal,snappedHorizontal, 0.1f, Time.deltaTime);
		animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
