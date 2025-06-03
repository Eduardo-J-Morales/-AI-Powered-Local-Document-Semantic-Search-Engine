from keybert import KeyBERT
from flask import Flask, jsonify, request
from flask_cors import CORS
import PyPDF2
import os
import io
app = Flask(__name__)
CORS(app)

kw_model = KeyBERT()

tags = []
@app.route('/keywords', methods=['POST'])
def get_text():
    file = request.files["file"]
    filename = file.filename

    pdf_reader = PyPDF2.PdfReader(io.BytesIO(file.read()))
    text = ""
    for page in pdf_reader.pages:
        text += page.extract_text() or ""
    
    base, _ = os.path.splitext(filename)
    output_path = f"{base}.txt"
    with open(output_path, "w") as f:
        f.write(text)


if __name__ == '__main__':
    app.run(debug=True)
