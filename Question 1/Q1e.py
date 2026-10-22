"""Q1e: Estimate 1D position and velocity with a Kalman filter."""

from pathlib import Path

import matplotlib
import numpy as np

# Use a non-interactive backend so the submission plot can be generated anywhere.
matplotlib.use("Agg")
import matplotlib.pyplot as plt


# Make the random simulation reproducible for the report.
np.random.seed(7)

# Simulation parameters.
dt = 0.1
num_steps = 50

# Constant-acceleration system model with position and velocity as the state.
A = np.array([[1, dt], [0, 1]])
B = np.array([[0.5 * dt**2], [dt]])
H = np.array([[1, 0]])

# Process and measurement noise covariances.
sigma_process = 0.5
sigma_measure = 2.0
Q = sigma_process**2 * np.array(
    [[dt**4 / 4, dt**3 / 2], [dt**3 / 2, dt**2]]
)
R = np.array([[sigma_measure**2]])

# Initial true state, estimated state, and estimate uncertainty.
x_true = np.array([[0.0], [1.0]])
x_est = np.array([[0.0], [0.0]])
P = np.eye(2)

true_positions = []
measured_positions = []
estimated_positions = []

for _ in range(num_steps):
    # Evolve the simulated object under constant acceleration and process noise.
    acceleration = 2.0
    process_noise = np.random.multivariate_normal([0.1, 0], Q).reshape(2, 1)
    x_true = A @ x_true + B * acceleration + process_noise

    # Observe position only, with normally distributed measurement noise.
    measurement_noise = np.random.normal(0, sigma_measure)
    z = H @ x_true + measurement_noise

    # Prediction step.
    x_pred = A @ x_est + B * acceleration
    P_pred = A @ P @ A.T + Q

    # Correction step: use the residual to update the predicted state.
    residual = z - H @ x_pred
    residual_covariance = H @ P_pred @ H.T + R
    kalman_gain = P_pred @ H.T @ np.linalg.inv(residual_covariance)
    x_est = x_pred + kalman_gain @ residual
    P = (np.eye(2) - kalman_gain @ H) @ P_pred

    true_positions.append(x_true[0, 0])
    measured_positions.append(z[0, 0])
    estimated_positions.append(x_est[0, 0])

# Plot and save the filter output beside this script.
t = np.arange(num_steps) * dt
plt.figure(figsize=(10, 5))
plt.plot(t, true_positions, "g-", label="True Position", linewidth=2)
plt.plot(t, measured_positions, "b*", label="Measured (Noisy)")
plt.plot(t, estimated_positions, "r.", label="KF Estimated Position")
plt.xlabel("Time [s]")
plt.ylabel("Position")
plt.title("1D Kalman Filter Simulation (Position-Velocity Model)")
plt.legend()
plt.grid(True)
plt.tight_layout()
plt.savefig(Path(__file__).with_name("Q1e_output.png"), dpi=180)

