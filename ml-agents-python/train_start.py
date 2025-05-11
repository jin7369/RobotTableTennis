import datetime
import subprocess

timestamp = datetime.datetime.now().strftime("%Y_%m_%d_%H_%M")
run_id = f"Serve{timestamp}"

subprocess.run([
    "mlagents-learn",
    "config/Serve.yaml",
    "--run_id", run_id,
    "--env=env/Serve05_11_2/Serve.exe",
    "--no-graphics"
])

subprocess.run([
    "mlagents-learn",
    "config/Serve.yaml",
    "--run_id", run_id,
    "--env=env/Serve05_11_3/Serve.exe",
    "--no-graphics"
])