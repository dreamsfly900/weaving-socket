package com.dahezhixin.haidilaomonitor.contentclass;

import java.io.Serializable;

/**
 * Created by hp on 15-11-23.
 */
public class LayoutCont implements Serializable {
    private String engName,chinName,coordinateOffset,groupName;
    private int exid,displayway;
    public LayoutCont() {
    }

    public LayoutCont(String chinName,String groupName) {
        this.chinName = chinName;
        this.groupName = groupName;
    }

    public LayoutCont( int exid,String engName, String chinName, int displayway, String coordinateOffset, String groupName) {
        this.engName = engName;
        this.chinName = chinName;
        this.coordinateOffset = coordinateOffset;
        this.groupName = groupName;
        this.exid = exid;
        this.displayway = displayway;
    }

    public String getEngName() {
        return engName;
    }

    public void setEngName(String engName) {
        this.engName = engName;
    }

    public String getChinName() {
        return chinName;
    }

    public void setChinName(String chinName) {
        this.chinName = chinName;
    }

    public String getGroupName() {
        return groupName;
    }

    public void setGroupName(String groupName) {
        this.groupName = groupName;
    }

    public String getCoordinateOffset() {
        return coordinateOffset;
    }

    public void setCoordinateOffset(String coordinateOffset) {
        this.coordinateOffset = coordinateOffset;
    }

    public int getExid() {
        return exid;
    }

    public void setExid(int exid) {
        this.exid = exid;
    }

    public int getDisplayway() {
        return displayway;
    }

    public void setDisplayway(int displayway) {
        this.displayway = displayway;
    }
}
