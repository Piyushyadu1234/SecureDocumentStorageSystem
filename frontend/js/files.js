const api = "https://localhost:7075/api";
const token = localStorage.getItem("token");

if (!token) {
    alert("You must login first");
    window.location.href = "index.html";
}

function uploadFile() {
    const fileInput = document.getElementById("fileInput");
    if (fileInput.files.length === 0) {
        alert("Please select a file");
        return;
    }
    const file = fileInput.files[0];
    const formData = new FormData();
    formData.append("file", file);

    fetch(`${api}/files/upload`, {
        method: "POST",
        headers: {
            "Authorization": `Bearer ${token}`
        },
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            alert(data.message || "File uploaded");
            listFiles();
        })
        .catch(err => alert("Upload failed: " + err.message));
}

function listFiles() {
    fetch(`${api}/files/list`, {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(res => res.json())
        .then(data => {
            const fileList = document.getElementById("fileList");
            fileList.innerHTML = "";
            data.forEach(file => {
                const item = document.createElement("li");
                item.innerHTML = `
                <span>${file.name}</span>
                <button onclick="downloadFile('${file.name}')" 
                        class="ml-2 text-blue-600 underline">Download</button>
            `;
                fileList.appendChild(item);
            });
        });
}

function downloadFile(filename) {
    fetch(`${api}/files/${encodeURIComponent(filename)}`, {
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(res => {
            if (!res.ok) throw new Error("Download failed");
            return res.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        })
        .catch(err => alert(err.message));
}

document.addEventListener("DOMContentLoaded", listFiles);
