using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

	public float JumpHeight = 4;
	//����
	public float Gravity=10;

	//
	private float yMove = 0;	// ��ֱ�ٶ�
	private Vector3 move = Vector3.zero; 

	private int m_AttackIdx = 0;

	//��ɫ������
	private CharacterController m_Controller;

	//��ɫ������
	private Animator m_Animator;

	private Player m_Player;

	private bool hasAdjustRotation;

	void Start ()
	{
		//��ȡ��ɫ������
		m_Controller=GetComponent<CharacterController>();
		m_Animator = GetComponentInChildren<Animator>();
		//
		m_Player=GetComponentInChildren<Player>();
	}

	void Update () {
		//ֻ�д�������״̬ʱ��ҿ����ж�
		MoveManager ();

	}
	 
	//�ƶ�����
	void MoveManager()
	{
		if (!m_Player.IsControllable)
			return;
		
		//�ƶ�����
		Vector3 mDir=Vector3.zero;

		// Read input
		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis("Vertical");


		bool bJump = CrossPlatformInputManager.GetButtonDown ("Jump");
		bool bAttack = CrossPlatformInputManager.GetButtonDown ("Fire1");
		bool bSkill1 = CrossPlatformInputManager.GetButtonDown ("Fire2");
		bool bSkill2 = CrossPlatformInputManager.GetButtonDown ("Fire3");
		if (bJump) {
			Debug.LogFormat("bJump {0}", bJump);
		}
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

			// camera 
			mDir = Camera.main.transform.forward * mDir.z + Camera.main.transform.right * mDir.x;
			mDir.y = 0;
		} else {
			// move stop
			isMoving = false;
		}


		if (m_Controller.isGrounded == false) {
			bJump = false;
			bAttack = false;
			bSkill1 = false;
			bSkill2 = false;
		}

		if (bJump) {
			yMove = JumpHeight;
			Debug.LogFormat("bJump {0}", bJump);
		}

		m_Animator.SetBool("isMoving", isMoving);
		//if (m_Animator.GetBool("isGrounded") != m_Controller.isGrounded) {
			m_Animator.SetBool("isGrounded", m_Controller.isGrounded);
		//}

		AnimatorStateInfo cur = m_Animator.GetCurrentAnimatorStateInfo(0);
		if (m_Animator.IsInTransition(0)) {
			// �ں�ʱ
			AnimatorStateInfo next = m_Animator.GetNextAnimatorStateInfo(0);
		} else {
			// ���ں�ʱ
			if (cur.IsName("run") && isMoving) {
				transform.forward = mDir;
				move.x = mDir.x * m_Player.Speed;
				move.z = mDir.z * m_Player.Speed;
				m_AttackIdx = 0;
				m_Animator.SetBool("bJump", bJump);
				if (bAttack) {
					m_AttackIdx = 1;
				}
				m_Animator.SetInteger("AttackIdx", m_AttackIdx);
				m_Animator.SetBool("bSkill1", bSkill1);
				m_Animator.SetBool("bHit", false);
				//Debug.Log("run");
			} else if (cur.IsName("idle")) {
				hasAdjustRotation = false;
				move.x = 0;
				move.z = 0;
				m_Animator.SetBool("bJump", bJump);
				m_AttackIdx = 0;
				if (bAttack) {
					m_AttackIdx = 1;
				}
				m_Animator.SetInteger("AttackIdx", m_AttackIdx);
				m_Animator.SetBool("bSkill1", bSkill1);
				//Debug.Log("idle");
			} else if (cur.IsName("jump")) {
				if (yMove > 0 && cur.normalizedTime < 0.5f) {	// ��ʼjump
					move *= 0.8f;
					move.y = yMove;		// ���ϳ��ٶ�
					yMove = 0;
					m_Animator.SetBool("isGrounded", false);
				}
				Debug.Log("jump " + cur.normalizedTime);
			} else if (cur.IsName("attack1_1")) {
				if (hasAdjustRotation == false) {
					m_Player.AutoRotateToEnemy();
					hasAdjustRotation = true;
				} else {
					// allow rotate while attack
					if (isMoving)  transform.forward = mDir;
				}
				move.x = 0;
				move.z = 0;
				if (bAttack && m_AttackIdx == 1) {
					m_AttackIdx ++;
					m_Animator.SetInteger("AttackIdx", m_AttackIdx);
				}
			} else if (cur.IsName("attack1_2")) {
				if (isMoving)  transform.forward = mDir;
				move.x = 0;
				move.z = 0;
				if (bAttack && m_AttackIdx == 2) {
					m_AttackIdx ++;
					m_Animator.SetInteger("AttackIdx", m_AttackIdx);
				}
			} else if (cur.IsName("attack1_3")) {
				if (isMoving)  transform.forward = mDir;
				move.x = 0;
				move.z = 0;
			} else if (cur.IsName("hit")) {
				move.x = 0;
				move.z = 0;
				m_Animator.SetBool("bHit", false);
			} else if (cur.IsName("skill1")) {
				if (hasAdjustRotation == false) {
					m_Player.AutoRotateToEnemy();
					hasAdjustRotation = true;
					m_Player.CastSkill(m_Player.SkillID);
				}
				move.x = 0;
				move.z = 0;
				m_Animator.SetBool("bSkill1", false);
			}
		}

		if (!m_Controller.isGrounded) {
			// �����½�
			move.y -= Gravity *Time.deltaTime;
		}

		CollisionFlags flags = m_Controller.Move(move * Time.deltaTime);
	}
 
}