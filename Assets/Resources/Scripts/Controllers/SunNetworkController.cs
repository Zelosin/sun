public class SunNetworkController : NetworkController {
    private void Start() {
        openPort(NetworkInfoStore.SUN_UDP_PORT);
        connectToPort(NetworkInfoStore.MOON_UDP_PORT);
        sendMessage(TagStore.CONNECTED);
    }
}