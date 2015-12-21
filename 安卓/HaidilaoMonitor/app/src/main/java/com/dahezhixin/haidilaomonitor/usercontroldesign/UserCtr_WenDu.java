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
public class UserCtr_WenDu extends LinearLayout {
    private TextView UCtrWendu_tv1, UCtrWendu_tv2;
    public UserCtr_WenDu(Context context) {
        super(context);
    }

    public UserCtr_WenDu(Context context, AttributeSet attrs) {
        super(context, attrs);
        View view = LayoutInflater.from(context).inflate(R.layout.userctr_wendu, this, true);
        UCtrWendu_tv1 = (TextView) view.findViewById(R.id.uctr_wendu_tv1);
        UCtrWendu_tv2 = (TextView) view.findViewById(R.id.uctr_wendu_tv2);
        TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.UCtrWenDu);
        CharSequence text1 = a.getText(R.styleable.UCtrWenDu_uctrwendu_title);
        CharSequence text2 = a.getText(R.styleable.UCtrWenDu_uctrwendu_dushu);

        if (text1 != null) {
            UCtrWendu_tv1.setText(text1);
            UCtrWendu_tv2.setText(text2);
        }
        a.recycle();
    }

    public CharSequence getUCtrWenDuTitle() {
        return UCtrWendu_tv1.getText();
    }
    public CharSequence getUCtrWenDuDushu() {
        return UCtrWendu_tv2.getText();
    }
    public void setUCtrWenDuTitle(CharSequence text1) {
        UCtrWendu_tv1.setText(text1);
    }
    public void setUCtrWenDuDushu(CharSequence text2){
        UCtrWendu_tv2.setText(text2);
    }
}
