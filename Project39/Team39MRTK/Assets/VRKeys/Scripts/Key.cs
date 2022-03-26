/**
 * Copyright (c) 2017 The Campfire Union Inc - All Rights Reserved.
 *
 * Licensed under the MIT license. See LICENSE file in the project root for
 * full license information.
 *
 * Email:   info@campfireunion.com
 * Website: https://www.campfireunion.com
 */

using UnityEngine;
using System.Collections;
using TMPro;

namespace VRKeys {

	/// <summary>
	/// An individual key in the VR keyboard.
	/// </summary>
	public class Key : MonoBehaviour {
		public Keyboard keyboard;

		public TextMeshPro label;

		public Material inactiveMat;

		public Material activeMat;

		public Material disabledMat;

		public Vector3 defaultPosition;

		public Vector3 indicatePosition;
		public Vector3 indicatePressPosition;


		public Vector3 pressedPosition;

		public Vector3 pressDirection = Vector3.down;
		public Vector3 indicateDirection = Vector3.up;

		public float pressMagnitude = 0.1f;

		public bool autoInit = false;

		private bool isPressing = false;
		public bool isIndicating = false;

		private bool disabled = false;

		protected MeshRenderer meshRenderer;

		private IEnumerator _ActivateFor;

		private IEnumerator _Press;
		private IEnumerator _Indicate;

		public KeyGroup keyGroup;
		public int keyRow;

		private void Awake () {
			meshRenderer = GetComponent<MeshRenderer> ();
	
			if (autoInit) {
				Init (transform.localPosition);
			}
		}
		private KeyGroup getKeyGroup() {
			if (label.text == "q" || label.text == "a" || label.text == "z") {
				return KeyGroup.YellowLeft;
			} else if (label.text == "w" || label.text == "s" || label.text == "x") {
				return KeyGroup.YellowRight;
			} else if (label.text == "e" || label.text == "d" || label.text == "c") {
				return KeyGroup.RedLeft;
			} else if (label.text == "r" || label.text == "f" || label.text == "v") {
				return KeyGroup.RedRight;
			} else if (label.text == "t" || label.text == "g" || label.text == "b") {
				return KeyGroup.GreenLeft;
			} else if (label.text == "y" || label.text == "h" || label.text == "n") {
				return KeyGroup.GreenMiddle;
			} else if (label.text == "u" || label.text == "j" || label.text == "m") {
				return KeyGroup.GreenRight;
			} else if (label.text == "i" || label.text == "k" || label.text == "p") {
				return KeyGroup.BlueLeft;
			} else if (label.text == "o" || label.text == "l") {
				return KeyGroup.BlueRight;
			}
			return KeyGroup.Default;
		}

		public int getKeyRow() {
			if ("qwertyuio".Contains(label.text)) {
				return 0;
			} else if ("asdfghjkl".Contains(label.text)) {
				return 1;
			} else if ("zxcvbnmp".Contains(label.text)) {
				return 2;
			}
			return -1;
		}

		public void initGroup() {
			this.keyGroup = getKeyGroup();
			this.keyRow = getKeyRow();

			switch (keyGroup) {
				case KeyGroup.YellowLeft:
					meshRenderer.material.SetColor("_Color", new Color32(139, 238, 220, 255));
					break;
				case KeyGroup.YellowRight:
					meshRenderer.material.SetColor("_Color", new Color32(139, 238, 220, 255));
					break;
				case KeyGroup.RedLeft:
					meshRenderer.material.SetColor("_Color", new Color32(255, 228, 131, 255));
					break;
				case KeyGroup.RedRight:
					meshRenderer.material.SetColor("_Color", new Color32(255, 228, 131, 255));
					break;
				case KeyGroup.GreenLeft:
					meshRenderer.material.SetColor("_Color", new Color32(255, 188, 151, 255));
					break;
				case KeyGroup.GreenMiddle:
					meshRenderer.material.SetColor("_Color", new Color32(255, 188, 151, 255));
					break;
				case KeyGroup.GreenRight:
					meshRenderer.material.SetColor("_Color", new Color32(255, 188, 151, 255));
					break;
				case KeyGroup.BlueLeft:
					meshRenderer.material.SetColor("_Color", new Color32(176, 203, 255, 255));
					break;
				case KeyGroup.BlueRight:
					meshRenderer.material.SetColor("_Color", new Color32(176, 203, 255, 255));
					break;
				case KeyGroup.Default:
					meshRenderer.material.SetColor("_Color", new Color32(229, 229, 229, 255));
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Initialize the key with a default position and pressed position.
		/// </summary>
		/// <param name="defaultPos">Default position.</param>
		public void Init (Vector3 defaultPos) {
			defaultPosition = defaultPos;
			pressedPosition = defaultPos + (Vector3.down * 0.01f);
			indicatePosition = defaultPos + (Vector3.up * 0.1f);
			indicatePressPosition = indicatePosition + (Vector3.up * 0.1f);
		}

		private void OnEnable () {
			isPressing = false;
			disabled = false;
			transform.localPosition = defaultPosition;
			meshRenderer.material = inactiveMat;

			OnEnableExtras ();
		}

		/// <summary>
		/// Override this to add custom logic on enable.
		/// </summary>
		protected virtual void OnEnableExtras () {
			//meshRenderer.material.SetColor("_Color", Color.red);
			initGroup();
		}

		public void OnTriggerEnter (Collider other) {
			if (isPressing || disabled || keyboard.disabled || !keyboard.initialized) {
				return;
			}

			Mallet mallet = other.gameObject.GetComponent<Mallet> ();
			if (mallet != null) {
				if (!mallet.isMovingDownward) {
					return;
				}

				if (mallet.hand == Mallet.MalletHand.Left && keyboard.leftPressing) {
					return;
				} else if (mallet.hand == Mallet.MalletHand.Right && keyboard.rightPressing) {
					return;
				}

				if (_Press != null && _Press.MoveNext ()) {
					StopCoroutine (_Press);
				}
				_Press = Press (other, mallet);
				StartCoroutine (_Press);
			}
		}

		private IEnumerator Press (Collider other, Mallet mallet) {
			isPressing = true;

			/*
			if (mallet.hand == Mallet.MalletHand.Left) {
				keyboard.leftPressing = true;
			} else if (mallet.hand == Mallet.MalletHand.Right) {
				keyboard.rightPressing = true;
			}
			*/

			//mallet.HandleTriggerEnter (this);
			HandleTriggerEnter (other);

			transform.localPosition = pressedPosition;

			yield return new WaitForSeconds (0.125f);

			transform.localPosition = defaultPosition;
			isPressing = false;

			/*
			if (mallet.hand == Mallet.MalletHand.Left) {
				keyboard.leftPressing = false;
			} else if (mallet.hand == Mallet.MalletHand.Right) {
				keyboard.rightPressing = false;
			}
			*/
		}

		/// <summary>
		/// Override this to handle trigger events. Only fires when
		/// a downward trigger event occurred from the collider
		/// matching keyboard.colliderName.
		/// </summary>
		/// <param name="other">Collider.</param>
		public virtual void HandleTriggerEnter (Collider other) {
			// Override me!
		}

		/// <summary>
		/// Show the active material for the specified length of time.
		/// </summary>
		/// <param name="seconds">Seconds.</param>
		public void ActivateFor (float seconds) {
			if (_ActivateFor != null && _ActivateFor.MoveNext ()) {
				StopCoroutine (_ActivateFor);
			}
			_ActivateFor = DoActivateFor (seconds);
			StartCoroutine (_ActivateFor);
		}

		private IEnumerator DoActivateFor (float seconds) {
			meshRenderer.material = activeMat;
			yield return new WaitForSeconds (seconds);

			meshRenderer.material = inactiveMat;
		}

		/// <summary>
		/// Disable the key.
		/// </summary>
		public virtual void Disable () {
			disabled = true;

			if (meshRenderer != null) {
				meshRenderer.material = disabledMat;
			}
		}

		/// <summary>
		/// Re-enable a disabled key.
		/// </summary>
		public virtual void Enable () {
			disabled = false;

			if (meshRenderer != null) {
				meshRenderer.material = inactiveMat;
			}
		}

		/// <summary>
		/// Update the key's label from a new language.
		/// </summary>
		/// <param name="translation">Translation object.</param>
		public virtual void UpdateLayout (Layout translation) {
			// Override me!
		}



		public void updatePosition() {
			if (isIndicating && isPressing) {
				transform.localPosition = indicatePressPosition;
			} else if (isIndicating) {
				transform.localPosition = indicatePosition;
			} else {
				transform.localPosition = defaultPosition;
			}
		}

		public void Indicate(bool indicate) {
			if (_Indicate != null && _Indicate.MoveNext ()) {
				StopCoroutine (_Indicate);
			}
			_Indicate = IndicateThread(indicate);
			StartCoroutine (_Indicate);
		}

		public IEnumerator IndicateThread(bool indicate) {
			if (indicate) {
				isIndicating = true;
				updatePosition();
			} else {
				yield return new WaitForSeconds (0.1f);
				isIndicating = false;
				updatePosition();
				keyboard.ResetKeyGroup();
			}
		}

		public void IndicatePress(bool press) {
			if (_Press != null && _Press.MoveNext ()) {
				StopCoroutine (_Press);
			}
			_Press = IndicateThreadPress(press);
			StartCoroutine (_Press);
		}

		public IEnumerator IndicateThreadPress(bool press) {
			if (press) {
				isPressing = true;
				updatePosition();
			} else {
				yield return new WaitForSeconds (0.1f);
				isPressing = false;
				updatePosition();
				keyboard.ResetRow();
			}
		}
	}
}