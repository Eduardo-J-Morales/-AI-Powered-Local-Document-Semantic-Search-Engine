from keybert import KeyBERT

kw_model = KeyBERT()
doc = "This text is related to the topic of biology and animals."
tags = kw_model.extract_keywords(doc)

print([tag for tag, score in tags])