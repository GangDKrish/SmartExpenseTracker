async function performLogin(endpoint, email, password, name) {
    try {
        const body = { email, password };
        if (name) body.name = name;
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        if (response.ok) {
            return { success: true, message: null };
        } else {
            const error = await response.json().catch(() => null);
            return { success: false, message: error?.message || 'Something went wrong. Please try again.' };
        }
    } catch {
        return { success: false, message: 'Unable to connect. Please try again.' };
    }
}
