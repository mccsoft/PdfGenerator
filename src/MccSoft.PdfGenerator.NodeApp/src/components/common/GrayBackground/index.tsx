import React, { ReactNode } from 'react';
import './styles.scss';

interface Props {
    children: ReactNode;
}

export const GrayBackground = (props: Props) => {
    return (
        <div className={'background-gray'}>
            {props.children}
        </div>
    );
};
