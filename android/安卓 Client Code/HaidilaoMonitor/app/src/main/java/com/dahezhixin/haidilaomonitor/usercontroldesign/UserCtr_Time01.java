package com.dahezhixin.haidilaomonitor.usercontroldesign;

import android.content.Context;
import android.content.res.TypedArray;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.dahezhixin.haidilaomonitor.R;

/**
 * Created by hp on 15-11-11.
 */
public class UserCtr_Time01 extends LinearLayout {
    private TextView time1_tv1, time1_tv2;
    public UserCtr_Time01(Context context) {
        super(context);
    }

    public UserCtr_Time01(Context context, AttributeSet attrs) {
        super(context, attrs);
        View view = LayoutInflater.from(context).inflate(R.layout.userctr_time01, this, true);
        time1_tv1 = (TextView) view.findViewById(R.id.uctr_time01_tv1);
        time1_tv2 = (TextView) view.findViewById(R.id.uctr_time01_tv2);
        TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.UCtrTime01);
        CharSequence text1 = a.getText(R.styleable.UCtrTime01_uctrtime_title);
        CharSequence text2 = a.getText(R.styleable.UCtrTime01_uctrtime_content);
        if (text1 != null) {
            time1_tv1.setText(text1);
            time1_tv2.setText(text2);
        }
        a.recycle();
    }
    public CharSequence getUCtrTimeTextTitle() {
        return time1_tv1.getText();
    }
    public CharSequence getUCtrTimeTextContent() {
        return time1_tv2.getText();
    }
    public void setUCtrTimeTextTitle(CharSequence text) {
        time1_tv1.setText(text);
    }
    public void setUCtrTimeTextContent(CharSequence text){
        time1_tv2.setText(text);
    }
}
