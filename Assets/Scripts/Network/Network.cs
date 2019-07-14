
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts {

	public enum NetworkMode {
		None,
		Send,
		Receive,
	}

	public struct NetworkPosition {

	}

	public class NetworkQueues {

	}

	public class UdpUtilities {

		private enum MessageType {
			Position,
		}

		public static void UdpLoop(UdpClient receivingUdpClient, ConcurrentQueue<Vector2> messageQueue) {

			//Creates an IPEndPoint to record the IP Address and port number of the sender. 
			// The IPEndPoint will allow you to read datagrams sent from any source.
			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

			Byte[] receiveBytes;
			// string returnData;

			Console.WriteLine("Started UDP listener");

			List<IPAddress> players = new List<IPAddress>();

			bool running = true;
			while (running) {
				try {
					// Blocks until a message returns on this socket from a remote host.
					receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);


					IPAddress playerAddress = RemoteIpEndPoint.Address;
					int playerPort = RemoteIpEndPoint.Port;

					MessageType message = MessageType.Position;
					Vector2 position = new Vector2();

					for (int i = 0; i < receiveBytes.Length; i++) {
						if (i == 0) {
							switch (receiveBytes[i]) {
								case 0:
									// message = MessageType.Position;
									break;
								case 1:
									message = MessageType.Position;
									break;
								default:
									break;
							}

						} else {
							switch (message) {
								case MessageType.Position:
									// TODO: parse Vector2
									// IDEA: add byte multiplied by digit worth to respective axis, up to float precision
									break;
								default:
									break;
							}
						}
					}

					switch (message) {
						case MessageType.Position:
							// TODO: send parsed Vector2
							messageQueue.Enqueue(position);
							break;
						default:
							break;
					}




					/* 
					returnData = Encoding.ASCII.GetString(receiveBytes);

					Console.WriteLine(
						"This is the message you received " +
						returnData.ToString()
					);

					Console.WriteLine(
						"This message was sent from " +
						RemoteIpEndPoint.Address.ToString() +
						" on their port number " +
						RemoteIpEndPoint.Port.ToString()
					);
					*/

				} catch (Exception e) {
					Console.WriteLine("UDP listener error: " + e.ToString());
					running = false;
				}
			}

			Console.WriteLine("Stopped UDP listener");

		}
	}
}
