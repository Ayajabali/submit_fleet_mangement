import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private socket: WebSocket;

  private messageSubject = new Subject<string>();

  constructor() {
    this.socket = new WebSocket('ws://localhost:8181'); 
    this.socket.onmessage = (event) => {
      this.messageSubject.next(event.data);
    };

    this.socket.onopen = () => {
      console.log('WebSocket connection opened');
    };

    this.socket.onclose = () => {
      console.log('WebSocket connection closed');
    };

    this.socket.onerror = (error) => {
      console.error('WebSocket error', error);
    };
  }

  onMessage(): Observable<string> {
    return this.messageSubject.asObservable();
  }

  sendMessage(message: string): void {
    this.socket.send(message);
  }

  close(): void {
    this.socket.close();
  }
}
