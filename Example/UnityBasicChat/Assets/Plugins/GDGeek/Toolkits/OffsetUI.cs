/*-----------------------------------------------------------------------------
The MIT License (MIT)

This source file is part of GDGeek
    (Game Develop & Game Engine Extendable Kits)
For the latest info, see http://gdgeek.com/

Copyright (c) 2014-2015 GDGeek Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
using UnityEngine;
using System.Collections;
namespace GDGeek{
	


	[ExecuteInEditMode]
	public class OffsetUI : MonoBehaviour {
		public Vector2 _original;
		public RectTransform _rect;
		public bool _enable = false;
		// Use this for initialization
		private void refersh(){
			if (_rect == null || _original.x == 0 || _original.y == 0) {
				return;		
			}
			if (_enable) {
					float rx = Screen.width / _original.x;
					float ry = Screen.height / _original.y;
					if (rx < ry) {
							float r = rx;
			
							float cha = (1f - r) * Screen.height;
							float x = (1f - r) / 2 * Screen.width;
							float y = cha / 2f;
			
							this.gameObject.transform.localScale = new Vector3 (r, r, 1);
							this.gameObject.transform.localPosition = new Vector3 (x, y, 0);
							_rect.sizeDelta = new Vector2 ((_original.x - Screen.width), cha / r);
					} else {
							float r = ry;
			
							float cha = (1f - r) * Screen.width;
							float y = (1 - r) / 2 * Screen.height;
							float x = cha / 2f;
			
							this.gameObject.transform.localScale = new Vector3 (r, r, 1);
							this.gameObject.transform.localPosition = new Vector3 (x, y, 0);
							_rect.sizeDelta = new Vector2 (cha / r, (_original.y - Screen.height));
					}		
			} else {
				this.gameObject.transform.localScale = new Vector3 (1, 1, 1);
				this.gameObject.transform.localPosition = new Vector3 (0, 0, 0);
				_rect.sizeDelta = new Vector2 (0, 0 );
			}

		}
		void Start () {
			refersh ();

		}
#if UNITY_EDITOR
		void Update(){
			
			refersh ();
		}
#endif

	}
}
