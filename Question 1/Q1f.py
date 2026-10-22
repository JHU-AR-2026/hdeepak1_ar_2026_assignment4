"""Q1f: Run the same 1D model without measurement correction."""

from pathlib import Path

import matplotlib
import numpy as np

# Use a non-interactive backend so the submission plot can be generated anywhere.
matplotlib.use("Agg")
import matplotlib.pyplot as plt


# Match Q1e's random sequence for a direct comparison between the plots.
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
R = np.array([[sigma_measure**2]])  # Retained from Q1e but unused without correction.

# Initial true state, estimated state, and estimate uncertainty.
x_true = np.array([[0.0], [1.0]])
x_est = np.array([[0.0], [0.0]])
P = np.eye(2)

true_positions = []
measured_positions = []
predicted_positions = []

for _ in range(num_steps):
    # Evolve the same noisy system used in Q1e.
    acceleration = 2.0
    process_noise = np.random.multivariate_normal([0.1, 0], Q).reshape(2, 1)
    x_true = A @ x_true + B * acceleration + process_noise

    # Generate measurements for comparison, but never feed them to the estimate.
    measurement_noise = np.random.normal(0, sigma_measure)
    z = H @ x_true + measurement_noise

    # Prediction step only; no residual, Kalman gain, or measurement correction.
    x_pred = A @ x_est + B * acceleration
    P_pred = A @ P @ A.T + Q
    x_est = x_pred
    P = P_pred

    true_positions.append(x_true[0, 0])
    measured_positions.append(z[0, 0])
    predicted_positions.append(x_est[0, 0])

# Plot and save the prediction-only output beside this script.
t = np.arange(num_steps) * dt
plt.figure(figsize=(10, 5))
plt.plot(t, true_positions, "g-", label="True Position", linewidth=2)
plt.plot(t, measured_positions, "b*", label="Measured (Unused)")
plt.plot(t, predicted_positions, "r.", label="Prediction Only")
plt.xlabel("Time [s]")
plt.ylabel("Position")
plt.title("1D Kalman Filter Without Measurement Correction")
plt.legend()
plt.grid(True)
plt.tight_layout()
plt.savefig(Path(__file__).with_name("Q1f_output.png"), dpi=180)

