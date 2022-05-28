using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityGGPO;

public class GgpoPerformancePanel : MonoBehaviour, IPerfUpdate
{
    public Rect fairnessRect;
    public Rect networkRect;

    public bool toggle; // @todo
    public bool random; // @todo
    public int _num_players = 2;
    public int step = 1;

    private const int MAX_GRAPH_SIZE = 4096;
    private const int MAX_FAIRNESS = 20;
    private const int MAX_PLAYERS = 4;

    private string networkLag;
    private string frameLag;
    private string bandWidth;
    private string localAhead;
    private string remoteAhead;

    private int _last_text_update_time;
    private int _first_graph_index = 0;
    private int _graph_size = 0;

    private int[][] _ping_graph;
    private int[][] _local_fairness_graph;
    private int[][] _remote_fairness_graph;

    private int[] _fairness_graph;
    private int[] _remote_queue_graph;
    private int[] _send_queue_graph;

    private Color[] _fairness_pens = new Color[] { Color.blue, Color.grey, Color.red, Color.magenta };
    private List<Vector2> pt = new List<Vector2>();

    public void Setup()
    {
        _fairness_graph = new int[MAX_GRAPH_SIZE];
        _remote_queue_graph = new int[MAX_GRAPH_SIZE];
        _send_queue_graph = new int[MAX_GRAPH_SIZE];

        _ping_graph = new int[MAX_PLAYERS][];
        _local_fairness_graph = new int[MAX_PLAYERS][];
        _remote_fairness_graph = new int[MAX_PLAYERS][];

        for(int i=0; i<MAX_PLAYERS; ++i)
        {
            _ping_graph[i] = new int[MAX_GRAPH_SIZE];
            _local_fairness_graph[i] = new int[MAX_GRAPH_SIZE];
            _remote_fairness_graph[i] = new int[MAX_GRAPH_SIZE];
        }
    }

    void DrawGraph(Rect di, Color pen, int[] graph, int count, int min, int max)
    {
        pt.Clear();

        for(int i=0; i<count; i += step)
        {
            float y = Mathf.InverseLerp(min, max, graph[(_first_graph_index + i) % MAX_GRAPH_SIZE]);

            pt.Add(new Vector2(Mathf.Lerp(di.xMin, di.xMax, (float)i / count), Mathf.Lerp(di.yMin, di.yMax, y)));
        }

        GuiHelper.DrawLine(pt.ToArray(), pen, 1);
    }

    void DrawGrid(Rect di)
    {
        GuiHelper.DrawRect(di, Color.gray);
    }

    void draw_network_graph_control(Rect di)
    {

    }

    private void draw_fairness_graph_control(Rect di)
    {

    }

    void IPerfUpdate.ggpoutil_perfmon_update(GGPONetworkStats[] statss)
    {
        int num_players = statss.Length;
        int i;

        if (_graph_size < MAX_GRAPH_SIZE)
        {
            i = _graph_size;
        }
        else
        {
            i = _first_graph_index;
            _first_graph_index = (_first_graph_index + 1) % MAX_GRAPH_SIZE;
        }

        for(int j=0; j< num_players; j++)
        {
            UpdateStats(i, j, statss[j]);
        }
    }

    private void UpdateStats(int i, int j, GGPONetworkStats stats)
    {
        //Random Graphs
        if (j == 0)
        {
            _remote_queue_graph[i] = stats.recv_queue_len;
            _send_queue_graph[i] = stats.send_queue_len;
        }

        //Ping
        _ping_graph[j][i] = stats.ping;

        //Frame Advantage
        _local_fairness_graph[j][i] = stats.local_frames_behind;
        _remote_fairness_graph[j][i] = stats.remote_frames_behind;
        if (stats.local_frames_behind < 0 && stats.remote_frames_behind < 0)
        {
            _fairness_graph[i] = Mathf.Abs(Mathf.Abs(stats.local_frames_behind) - Mathf.Abs(stats.remote_frames_behind));
        }
        else if(stats.local_frames_behind >0 && stats.remote_frames_behind > 0)
        {
            _fairness_graph[i] = 0;
        }
        else
        {
            _fairness_graph[i] = Mathf.Abs(stats.local_frames_behind) + Mathf.Abs(stats.remote_frames_behind);
        }

        int now = Utils.TimeGetTime();
        if (now > _last_text_update_time + 500)
        {
            networkLag = $"{stats.ping} ms";
            frameLag = $"{((stats.ping != 0) ? stats.ping * 60f / 1000f : 0f)} frames";
            bandWidth = $"{stats.kbps_sent / 8f} kilobytes/sec";
            localAhead = $"{stats.local_frames_behind} frames";
            remoteAhead = $"{stats.remote_frames_behind} frames";
            _last_text_update_time = now;
        }
    }

    private void RandomGraph(int[] graph, int size, int min, int max)
    {
        for(int i=0; i<size; ++i)
        {
            graph[i] = Random.Range(min, max);
        }
    }
}
