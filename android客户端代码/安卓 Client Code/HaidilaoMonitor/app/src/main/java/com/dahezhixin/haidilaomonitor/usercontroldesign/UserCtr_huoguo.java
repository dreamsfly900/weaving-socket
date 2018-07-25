package com.dahezhixin.haidilaomonitor.usercontroldesign;

import android.content.Context;
import android.content.res.TypedArray;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.LinearLayout;

import com.dahezhixin.haidilaomonitor.R;

/**
 * Created by hp on 15-12-2.
 */
public class UserCtr_huoguo extends LinearLayout{
    UserCtr_WenDu hg_myline0;
    UserCtr_Time01 hg_myline1,hg_myline2,hg_myline3;
    public UserCtr_huoguo(Context context) {
        super(context);
        initview();
    }

    public UserCtr_huoguo(Context context, AttributeSet attrs) {
        super(context, attrs);
        initview();

        TypedArray a = getContext().obtainStyledAttributes(attrs, R.styleable.UserCtrHuoGuo);
        CharSequence line0_tv1 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line0_tv1);
        CharSequence line0_tv2 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line0_tv2);
        if (line0_tv1 != null) {
            hg_myline0.setUCtrWenDuTitle(line0_tv1);
            hg_myline0.setUCtrWenDuDushu(line0_tv2);
        }
        CharSequence line1_tv1 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line1_tv1);
        CharSequence line1_tv2 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line1_tv2);
        if (line0_tv1 != null) {
            hg_myline1.setUCtrTimeTextTitle(line1_tv1);
            hg_myline1.setUCtrTimeTextContent(line1_tv2);
        }
        CharSequence line2_tv1 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line2_tv1);
        CharSequence line2_tv2 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line2_tv2);
        if (line0_tv1 != null) {
            hg_myline2.setUCtrTimeTextTitle(line2_tv1);
            hg_myline2.setUCtrTimeTextContent(line2_tv2);
        }
        CharSequence line3_tv1 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line3_tv1);
        CharSequence line3_tv2 = a.getText(R.styleable.UserCtrHuoGuo_uctrhg_line3_tv2);
        if (line0_tv1 != null) {
            hg_myline3.setUCtrTimeTextTitle(line3_tv1);
            hg_myline3.setUCtrTimeTextContent(line3_tv2);
        }
        a.recycle();
    }
    private void initview() {
        View view = LayoutInflater.from(getContext()).inflate(R.layout.userctr_huoguo, this, true);
        hg_myline0 = (UserCtr_WenDu) view.findViewById(R.id.myline0);
        hg_myline1 = (UserCtr_Time01) view.findViewById(R.id.mylinecontent1);
        hg_myline2 = (UserCtr_Time01) view.findViewById(R.id.mylinecontent2);
        hg_myline3 = (UserCtr_Time01) view.findViewById(R.id.mylinecontent3);

    }
    public CharSequence getHg_line0_tv1() {
        return hg_myline0.getUCtrWenDuTitle();
    }
    public CharSequence getHg_line1_tv1() {
        return hg_myline1.getUCtrTimeTextTitle();
    }
    public CharSequence getHg_line2_tv1() {
        return hg_myline2.getUCtrTimeTextTitle();
    }
    public CharSequence getHg_line3_tv1() {
        return hg_myline3.getUCtrTimeTextTitle();
    }

    public void setHg_line0_tv1(CharSequence text01) {
        hg_myline0.setUCtrWenDuTitle(text01);
    }
    public void setHg_line0_tv2(CharSequence text02) {
        hg_myline0.setUCtrWenDuDushu(text02);
    }
    public void setHg_line1_tv1(CharSequence text11) {
        hg_myline1.setUCtrTimeTextTitle(text11);
    }
    public void setHg_line1_tv2(CharSequence text12) {
        hg_myline1.setUCtrTimeTextContent(text12);
    }
    public void setHg_line2_tv1(CharSequence text21) {
        hg_myline2.setUCtrTimeTextTitle(text21);
    }
    public void setHg_line2_tv2(CharSequence text22) {
        hg_myline2.setUCtrTimeTextContent(text22);
    }
    public void setHg_line3_tv1(CharSequence text31) {
        hg_myline3.setUCtrTimeTextTitle(text31);
    }
    public void setHg_line3_tv2(CharSequence text32) {
        hg_myline3.setUCtrTimeTextContent(text32);
    }


}
