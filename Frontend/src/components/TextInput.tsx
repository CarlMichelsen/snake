import { type FC } from "react";

type TextInputProps = {
    id?: string;
    onChange: (text: string) => void;
    value: string;
    placeholder?: string;
    className?: string;
    onEnter?: () => void;
}

const TextInput: FC<TextInputProps> = ({ id, onChange, value, placeholder, className, onEnter }) => {
    return <input
        id={id}
        type="text"
        value={value}
        placeholder={placeholder}
        onKeyDown={e => {
            if (e.key == 'Enter' && onEnter) {
                onEnter();
                e.preventDefault();
            }
        }}
        onChange={e => onChange(e.target.value)}
        className={`block dark:bg-neutral-800 dark:focus:bg-neutral-600 bg-blue-200 focus:bg-neutral-200 text-lg px-1 py-0.5 outline-none text-center ${className ?? ""}`} />
}

export default TextInput;