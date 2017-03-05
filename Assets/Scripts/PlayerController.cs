using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public enum CharaterState {
	IDLE=0,
	IDLE2,
	RUN,
	JUMP,
	ATTACK1_1,
	ATTACK1_2,
	ATTACK1_3,
	ATTACK2,
	DEATH,
}

public class PlayerController : MonoBehaviour {
 
	//�ƶ��ٶ�
	public float MoveSpeed=1.5F;
	//�����ٶ�
	public float RunSpeed=4.5F;
	//��ת�ٶ�
	public float RotateSpeed=30;
	//����
	public float Gravity=20;
	//�������
	private Animation mAnim;
	//�ٶ�
	private float mSpeed;

	// state
	private CharaterState mCharaterState;

	//��ɫ������
	private CharacterController mController;
	 
	void Start ()
	{
		//��ȡ��ɫ������
		mController=GetComponent<CharacterController>();
		//��ȡ�������
		mAnim=GetComponentInChildren<Animation>();
	}
	 
	void Update ()
	{
		//ֻ�д�������״̬ʱ��ҿ����ж�
		MoveManager ();

	}
	 
	//�ƶ�����
	void MoveManager()
	{
		//�ƶ�����
		Vector3 mDir=Vector3.zero;

		// Read input
		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis("Vertical");

		bool bJump = CrossPlatformInputManager.GetButtonDown ("Jump");
		bool bAttack = CrossPlatformInputManager.GetButtonDown ("Fire1");

		bool isMoving = false;

		if (Mathf.Abs (horizontal) > 0.01f || Mathf.Abs (vertical) > 0.01f) {
			// move
			isMoving = true;
			mDir.x = horizontal;
			mDir.z = vertical;	

			// normalize input if it exceeds 1 in combined length:
			if (mDir.sqrMagnitude > 1) {
				mDir.Normalize();
			}
		} else {
			// move stop
			isMoving = false;
		}

		if (mController.isGrounded) {
			if (bJump) {
				mDir.y = 20;
				// ˮƽ
				mAnim.Play ("jump");
			} else if (isMoving) {
				transform.forward = mDir;
				mDir = mDir * RunSpeed;
				mAnim.Play ("run");
			} else {
				mAnim.Play ("idle");
			}
		} else {
			mDir.x = 0;
			mDir.z = 0;
		}

		//������������
		//mDir=transform.TransformDirection(mDir);
		float y = mDir.y-Gravity *Time.fixedDeltaTime;
		mDir=new Vector3(mDir.x,y,mDir.z);
		mController.Move(mDir * Time.fixedDeltaTime);
	}
 
}