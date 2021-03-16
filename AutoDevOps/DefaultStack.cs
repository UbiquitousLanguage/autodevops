using Pulumi;

namespace AutoDevOps {
    public class AppStack : Stack {
        public AppStack() {
            var config   = new Config();
            var settings = new AutoDevOpsSettings(config);
            var autoDevOps = new AutoDevOps(settings);
        }
    }
}