<script>
	import { onMount } from 'svelte';
	let documents: any[] = [];
	let selectedFile: File | null = null;
	let searchQuery = '';
	let selectedDoc: any = null;
	let tagInput = '';
	let isUploading = false;
	let aiSuggestedTags: string[] = [];
	let userTags: string[] = [];

	// Fetch all documents on mount
	onMount(async () => {
		await fetchDocuments();
	});

	async function fetchDocuments() {
		const res = await fetch('/api/documents');
		documents = await res.json();
	}

	async function uploadDocument() {
		if (!selectedFile) return;
		isUploading = true;
		cont formData = new FormData();
		formData.append('file', selectedFile)

		const res = await fetch('/api/documents/upload', {
			method: 'POST',
			body: formData
		});
		isUploading = false
		selectedFile = null
		await fetchDocuments
	}

</script>
<main>
	
</main>