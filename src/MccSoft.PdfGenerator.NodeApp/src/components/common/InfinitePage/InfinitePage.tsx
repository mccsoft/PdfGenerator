import React, { ReactNode } from 'react';
import { PageOrientation } from '~utils/PageOrientation';
import './styles.scss';

type Props = {
    children: ReactNode;
    pageOrientation: PageOrientation;
};

/**
 * This component is needed for styles in development mode
 * and for correct splitting components while report is printed.
 */
export const InfinitePage = ({ children, pageOrientation }: Props) => {
    const style = pageOrientation === PageOrientation.Landscape
        ? 'landscape-infinity-page'
        : 'portrait-infinity-page';

    return (
        <table className={style}>
            <tbody className={'infinity-page-content'}>
            {
                React.Children.map(children, (child, index) => (
                    <tr key={index} className={'infinity-page-content-tr'}>
                        <td className={'infinity-page-content-td'}>
                            {child}
                        </td>
                    </tr>
                ))
            }
            </tbody>
        </table>
    );
};
