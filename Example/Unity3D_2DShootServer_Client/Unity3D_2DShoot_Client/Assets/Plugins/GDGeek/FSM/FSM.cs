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
using System.Collections.Generic;
namespace GDGeek{
	public class FSM {

		private Dictionary<string, State> states_ = new Dictionary<string, State>();
		private List<State> currState_ = new List<State>();
		private bool debug_ = false;
		public FSM(bool debug = false){
			debug_ = debug;
			State root = new State();
			root.name = "root";
			this.states_["root"]  = root;
			this.currState_.Add(root);
		}
		public void addState(string stateName, State state){
		
			this.addState (stateName, state, "");		
		}

		public void addState(string stateName, string defSubState, State state){
			this.addState (stateName, state, "");		
			state.defSubState = defSubState;
		}
		public void addState(string stateName, string defSubState, State state, string fatherName){
			this.addState (stateName, state, fatherName);		
			state.defSubState = defSubState;
		}

		public void addState(string stateName, State state, string fatherName){	

			if(fatherName == ""){
				state.fatherName = "root";
			}
			else{
				state.fatherName = fatherName;
			}
			state.getCurrState = delegate (string name){	
				for(int i = 0; i< this.currState_.Count; ++i){
					State s = this.currState_[i] as State;
					if(s.name == name)
					{
						return s;
					}
				}	
				return null;
			};
			if (this.states_.ContainsKey (state.fatherName)) {
				if(string.IsNullOrEmpty(this.states_[state.fatherName].defSubState)){
					this.states_[state.fatherName].defSubState = stateName;
				}
			}
			state.name = stateName;
			this.states_[stateName] = state;

		}

	
		public void translation(string name)
		{

			
			if (!this.states_.ContainsKey(name))//if no target return!
			{
				return;
			}

			State target = this.states_[name];//target state
			while (!string.IsNullOrEmpty (target.defSubState) && this.states_.ContainsKey(target.defSubState)) {
				target = this.states_[target.defSubState];
			}
			
			//if current, reset
			if(target == this.currState_[this.currState_.Count-1])
			{
				target.over();
				target.start();
				return;
			}
			
			
			
			State publicState = null;
			
			List<State> stateList = new List<State>();
			
			State tempState = target;
			string fatherName = tempState.fatherName;
			
			//do loop 
			while(tempState != null)
			{
				//reiterator current list
				for(var i = this.currState_.Count -1; i >= 0; i--){
					State state = this.currState_[i] as State;
					//if has public 
					if(state == tempState){
						publicState = state;	
						break;
					}
				}
				
				//end
				if(publicState != null){
					break;
				}
				
				//else push state_list
				stateList.Insert(0, tempState);
				//state_list.unshift(temp_state);
				
				if(fatherName != ""){
					tempState = this.states_[fatherName] as State;
					fatherName = tempState.fatherName;
				}else{
					tempState = null;
				}
				
			}
			//if no public return
			if (publicState == null){
				return;
			}
			
			List<State> newCurrState = new List<State>();
			bool under = true;
			//-- 析构状态
			for(int i2 = this.currState_.Count -1; i2>=0; --i2)
			{
				State state2 = this.currState_[i2] as State;
				if(state2 == publicState)
				{
					under = false;
				}
				if(under){
					state2.over();
				}
				else{
					newCurrState.Insert(0, state2);
				}
				
			}
			
			
			//-- 构建状态
			for(int i3 = 0; i3 < stateList.Count; ++i3){
				State state3 = stateList[i3] as State;
				state3.start();

				newCurrState.Add(state3);
			}

			this.currState_ = newCurrState;
			string outs = "";
			for(int i = 0; i < currState_.Count; ++i){
				outs+= ":" + currState_[i].name;
			}
			
//			Debug.LogWarning (outs);

		}


		
		public State getCurrState(string name)
		{
			var self = this;
			for(var i =0; i< self.currState_.Count; ++i)
			{
				State state = self.currState_[i] as  State;
				if(state.name == name)
				{
					return state;
				}
			}
			
			return null;
			
		}
		
		public void init(string state_name){
			var self = this;

			self.translation(state_name);
		}
		
		
		public void shutdown(){
			for(int i = this.currState_.Count-1; i>=0; --i){
				State state =  this.currState_[i] as State;
				state.over();
			}
			this.currState_ = null;  
		}

		public void post(string msg){
			FSMEvent evt = new FSMEvent();

			evt.msg = msg;
			this.postEvent(evt);
		}
		public void postEvent(FSMEvent evt){

			string outs = "";
			for(int i = 0; i < currState_.Count; ++i){
				outs+= ":" + currState_[i].name;
			}
			
			Debug.Log (outs);


			for(int i =0; i< this.currState_.Count; ++i){
				State state = this.currState_[i] as State;
				if (debug_) {
					Debug.Log ("msg_post" + evt.msg+ state.name);			
				}
				string stateName = state.postEvent(evt) as string;
				if(stateName != ""){
					this.translation(stateName);
					break;
				}
			}
		}
	}
}
