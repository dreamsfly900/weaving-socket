package com.dahezhixin.haidilaomonitor.contentclass;

/**
 * Created by hp on 15-12-3.
 */
public class myData {
    String mytitle;
    String myvalue;

    public myData() {
    }

    public myData(String mytitle, String myvalue) {
        this.mytitle = mytitle;
        this.myvalue = myvalue;
    }

    public String getMyvalue() {
        return myvalue;
    }

    public void setMyvalue(String myvalue) {
        this.myvalue = myvalue;
    }

    public String getMytitle() {
        return mytitle;
    }

    public void setMytitle(String mytitle) {
        this.mytitle = mytitle;
    }
}
