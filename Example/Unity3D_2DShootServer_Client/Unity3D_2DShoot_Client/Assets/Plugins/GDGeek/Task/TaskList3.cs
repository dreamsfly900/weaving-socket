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
	
	public class TaskList3:Task{
		private Task begin_ = null;
		private Task end_ = null;
		private List<Task> other_ = new List<Task>(); 
		private bool isOver_ = false;
		private bool isCompleted_ = false;
		private bool forceExit_ = false;
		public void forceQuit(){
			forceExit_ = true;
			Debug.LogWarning ("force exit!");
		}
		
		public TaskList3(){
			this.init = this.initImpl;
			this.isOver = this.isOverImpl;
		}
		
		public void push(Task task){
			if(this.isCompleted_){
				Debug.LogError("list task is completed!");
			}
			if(this.begin_ == null && this.end_ == null)
			{
				this.begin_ = task;
				this.end_ = task;
			}else{
				Task end = this.end_;
				TaskShutdown oShutdown = end.shutdown;
				end.shutdown = delegate(){
					oShutdown();
					if(!forceExit_){
						TaskManager.Run(task);
					}else{
						this.isOver_ = true;
					}
				};
				other_.Add(this.end_);
				this.end_ = task;
			}
		}
		
		public void initImpl(){
			if(this.isCompleted_ == false && this.end_!=null){
				TaskManager.PushBack(this.end_, delegate(){this.isOver_ = true;});
				this.isCompleted_ = true;
			}
			forceExit_ = false;
			if(this.begin_ != null){ 
				this.isOver_ = false;
				TaskManager.Run(begin_); 
			}else{
				this.isOver_ = true;
			} 
		}
		
		
		public bool isOverImpl(){
			return this.isOver_;
		}
	};
}
