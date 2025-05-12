import tkinter as tk
import os
from tkinter import filedialog, scrolledtext
import subprocess
import threading
class Trainer:
    def __init__(self):
        self.mlagents_process = None
        self.root = tk.Tk()
        self.root.title("ML-Agent Trainer")

        self.python_path_var = tk.StringVar()
        self.env_path_var = tk.StringVar()
        self.config_path_var = tk.StringVar()
        self.run_id_var = tk.StringVar()
        self.status_var = tk.StringVar()

        self.label(text="Python Executable (python.exe)")
        self.entry(textvariable=self.python_path_var)
        self.button("Browse", command=self.browse_python_path)

        self.label(text="Environment file(.exe)")
        self.entry(textvariable=self.env_path_var)
        self.button(text="Browse", command=self.browse_env_path)

        self.label(text="Config file(.yaml)")
        self.entry(textvariable=self.config_path_var)
        self.button(text="Browse", command=self.browse_config_path)

        self.label(text="Run ID:")
        self.entry(textvariable=self.run_id_var)

        self.button(text="Start", command=self.run_mlagents)
        self.button(text="Stop", command=self.stop_mlagents)
        tk.Label(self.root, textvariable=self.status_var, fg="blue").pack()


        self.output_text = scrolledtext.ScrolledText(self.root, height=20, width=60)
        self.output_text.pack(pady=5)



        self.root.mainloop()
    
    def label(self, text):
        tk.Label(self.root, text=text).pack()
    
    def entry(self, textvariable):
        tk.Entry(self.root, textvariable=textvariable, width=50).pack()
    
    def button(self, text, command):
        tk.Button(self.root, text=text, command=command).pack()


    def browse_env_path(self):
        path = filedialog.askopenfilename(filetypes=[("Unity Executable", "*.exe")])
        self.env_path_var.set(value=path)

    
    def browse_config_path(self):
        path = filedialog.askopenfilename(filetypes=[("YAML files", "*.yaml")])
        self.config_path_var.set(value=path)
    
    def browse_python_path(self):
        path = filedialog.askopenfilename(filetypes=[("Python Executable", "python.exe")])
        self.python_path_var.set(path)

    def run_mlagents(self):
        self.status_var.set(value="Starting...")
        python_path = self.python_path_var.get()
        if not os.path.exists(python_path):
            self.status_var.set("Invalid python.exe path")
            return
        env = self.env_path_var.get()
        config = self.config_path_var.get()
        run_id = self.run_id_var.get()

        if not env or not config or not run_id:
            self.status_var.set("Please fill all blank!")
            return

        cmd = [
            python_path, "-m",
            "mlagents.trainers.learn", 
            "--env", env,
            "--run-id", run_id,
            "--no-graphics",
            config
        ]
        def run_cmd():
            try:
                self.mlagents_process = subprocess.Popen(
                    cmd,
                    stdout=subprocess.PIPE,
                    stderr=subprocess.STDOUT,
                    universal_newlines=True
                )
                self.output_text.delete(1.0, tk.END)
                for line in self.mlagents_process.stdout:
                    self.output_text.insert(index=tk.END, chars=line)
                    self.output_text.see(index=tk.END)
                self.mlagents_process.wait()
                if self.mlagents_process.returncode == 0:
                    self.status_var.set("Train Completed Successfully")
                else:
                    self.status_var.set("Error - Check the log below")
            except FileNotFoundError:
                self.status_var.set("Can't find mlagents-learn command")
            except Exception as e:
                self.status_var.set(f"Exception : {e}")
        
        threading.Thread(target=run_cmd, daemon=True).start()
    def stop_mlagents(self):
        if self.mlagents_process and self.mlagents_process.poll() is None:
            self.mlagents_process.terminate()
            self.status_var.set("Train stopped")
        else:
            self.status_var.set("Train is already stopped or not running")

trainer = Trainer()