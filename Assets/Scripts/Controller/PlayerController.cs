using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
 
	//�ƶ��ٶ�
	public float MoveSpeed=1.5F;
	//�����ٶ�
	public float RunSpeed=4.5F;
	//��ת�ٶ�
	public float RotateSpeed=30;
	//����
	public float Gravity=20;
	//�ٶ�
	private float mSpeed;

	//
	private float yMove = 0;	// ��ֱ�ٶ�
	private Vector3 move = Vector3.zero; 


	//��ɫ������
	private CharacterController m_Controller;

	private Player m_Player;

	void Start ()
	{
		//��ȡ��ɫ������
		m_Controller=GetComponent<CharacterController>();
		//
		m_Player=GetComponentInChildren<Player>();
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

		if (m_Controller.isGrounded) {

			if (m_Player.CharcterState == CharaterState.IDLE || m_Player.CharcterState == CharaterState.RUN) {
				move = mDir * RunSpeed;
			} else {
				// no move 
				isMoving = false;
			}
			if (bAttack) {
				move.x = 0;
				move.z = 0;
				m_Player.ActAttack1();
			} else if (bJump) {
				move.y = 6;		// ���ϳ��ٶ�
				m_Player.ActJump();
			} else if (isMoving) {
				transform.forward = mDir;
				if (m_Player.CharcterState != CharaterState.RUN) {
					m_Player.ActRun();
				}
			} else {
				move.x = 0;
				move.z = 0;
				// �ƶ�ֹͣ or ��Ծ�ŵ�
				if (m_Player.CharcterState == CharaterState.RUN || m_Player.CharcterState == CharaterState.JUMP) m_Player.ActIdle();
			}
		} else {
			// in air
		}

		// �����½�
		move.y -= Gravity *Time.fixedDeltaTime;
		m_Controller.Move(move * Time.fixedDeltaTime);
	}
 
}