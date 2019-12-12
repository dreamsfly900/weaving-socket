package weaving.socket;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
public class MainActivity extends Activity {
private Button test;

    NETDataSocket NDS;
    @Override

    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        test=(Button)findViewById(R.id.test);
        Log.i("ceshi","ceshi666");

        test.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                NDS=new NETDataSocket("192.168.1.127",8989);
            }
        });
    }
}
