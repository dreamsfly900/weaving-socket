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
	public class TaskRunner : MonoBehaviour {

		private Filter filter_ = new Filter();
		private ArrayList tasks_ = new ArrayList();

		public static TaskRunner Create(GameObject obj){
			TaskRunner runner = obj.GetComponent<TaskRunner>();
			if(runner == null){
				runner = obj.AddComponent<TaskRunner>();
			}
			return runner;
		}


		public void update(float d){
			var tasks = new ArrayList();
			for(var i=0; i< this.tasks_.Count; ++i){
				Task task = this.tasks_[i] as Task;
				task.update(d);
				if(!task.isOver()){
					tasks.Add(task);
				}else{
					task.shutdown();
				}
			}
			this.tasks_ = tasks;
		}
		/*
		protected virtual void  OnDestroy(){
			try{
				for(int i=0; i< this.tasks_.Count; ++i){
					Task task = this.tasks_[i] as Task;
					task.shutdown();
				}
				tasks_ = new ArrayList();
			}catch(System.Exception e){ 
				Debug.LogWarning(e.Message);			
			}
		}*/
		

		public void addTask(Task task){
			task.init();
			this.tasks_.Add(task);
		}
		
		protected virtual void Update() { 
			float d = filter_.interval(Time.deltaTime);
			this.update(d);
		}
	}
}
