import { Store } from 'redux';
import { ApplicationState } from './store';

export function ReduxHarness(store: Store<ApplicationState>, transformState: (s: ApplicationState) => any) {
    if (!transformState) {
        transformState = s => s;
    }

    function getQueryVariable(variable: string) {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (pair[0] == variable) { return pair[1]; }
        }
        return (false);
    }

    const sendCurrentState = () => {
        var state = store.getState();

        revision = revision + 1;
        var message = {
            type: 'redux-state',
            revision: revision,
            state: transformState(state)
        }

        if (socket.readyState == 1) {
            var json = JSON.stringify(message);
            console.log('Sending to engine: ' + json);
            socket.send(json);
        }
    }

    var revision = 1;

    var port = getQueryVariable('StorytellerPort');
    var wsAddress = "ws://127.0.0.1:5250";

    var socket = new WebSocket(wsAddress);
    
    socket.onclose = function () {
        console.log('The socket closed');
    };

    socket.onerror = function (evt) {
        console.error(JSON.stringify(evt));
    }

    socket.onmessage = function (evt) {
        if (evt.data == 'REFRESH') {
            window.location.reload();
            return;
        }

        if (evt.data == 'CLOSE') {
            window.close();
            return;
        }
        
        if (evt.data == 'FETCHSTATE') {
            sendCurrentState();
            return;
        }

        var message = JSON.parse(evt.data);
        console.log('Got: ' + JSON.stringify(message) + ' with topic ' + message.type);

        store.dispatch(message);
    };

    store.subscribe(sendCurrentState);


    var originalLog = console.log;
    console.log = function (msg) {
        originalLog(msg);

        var message = {
            type: 'console.log',
            text: msg
        }

        var json = JSON.stringify(message);
        socket.send(json);
    }

    var originalError = console.error;
    console.error = function (e) {
        originalError(e);

        var message = {
            type: 'console.error',
            error: e
        }

        var json = JSON.stringify(message);
        socket.send(json);
    }
}